using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Search.Discussions.Problems.Queries;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.Search.Documents;
using SNS.Infrastructure.Search.Abstractions;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Infrastructure.Search.Services;


public class ProblemSearchService : IProblemSearchService
{
    private readonly IElasticDocumentService<ProblemDocument> _elasticBaseService;
    private const string IndexName = "sns_problems";

    public ProblemSearchService(IElasticDocumentService<ProblemDocument> elasticBaseService)
    {
        _elasticBaseService = elasticBaseService;
    }

    // ==========================================
    // 1. البحث العادي (Search & Filter)
    // ==========================================
    public async Task<SearchResult<ProblemDocument>> SearchProblemsAsync(ProblemSearchQuery query, CancellationToken cancellationToken = default)
    {
        var mustQueries = new List<Query>();
        var filterQueries = new List<Query>();


        // 2. البحث النصي (Title & ContentManagement)
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            mustQueries.Add(new MultiMatchQuery
            {
                Query = query.SearchTerm,
                // إعطاء وزن (Boost) لعنوان المشكلة أعلى من محتواها، والبحث أيضاً داخل البلوكات
                Fields = new[] { "title^3.0", "topTwoContentBlock.content" },
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        // 3. الفلترة حسب الصعوبة (نفترض أن الـ Enums تُخزن كأرقام، إذا كانت نصوص استخدم ToString)
        if (query.Level.HasValue)
        {
            filterQueries.Add(new TermQuery { Field = "level", Value = (int)query.Level.Value });
        }

        // 4. الفلترة حسب الحالة (محلولة، مفتوحة..)
        if (query.Status.HasValue)
        {
            filterQueries.Add(new TermQuery { Field = "status", Value = (int)query.Status.Value });
        }

        // 5. الفلترة حسب التاريخ (نطاق زمني)
        if (query.MinCreatedAt.HasValue || query.MaxCreatedAt.HasValue)
        {
            filterQueries.Add(new DateRangeQuery
            {
                Field = "createdAt",
                Gte = query.MinCreatedAt,
                Lte = query.MaxCreatedAt
            });
        }

        return await _elasticBaseService.SearchAsync(IndexName, s => s
            .From((query.Page - 1) * query.PageSize)
            .Size(query.PageSize)
            .Query(q => q
                .Bool(b => b
                    .Must(mustQueries)
                    .Filter(filterQueries)
                )
            )
            .Sort(sort => sort
                .Score() // الترتيب حسب مدى تطابق النص أولاً
                .Field(f => f.UpVotesCount, fs => fs.Order(SortOrder.Desc)) // ثم حسب عدد الأصوات
                .Field(f => f.CreatedAt, fs => fs.Order(SortOrder.Desc))  // ثم الأحدث
            ),
            cancellationToken);
    }

    // ==========================================
    // 2. خوارزمية الـ Feed (Personalized Feed)
    // ==========================================
    public async Task<SearchResult<ProblemDocument>> GetProblemFeedAsync(ProblemFeedParameter request, CancellationToken cancellationToken = default)
    {
        var shouldQueries = new List<Query>();
        var filterQueries = new List<Query>();
        var mustNotQueries = new List<Query>();

        // 🟢 Filters (شروط أساسية يجب أن تتحقق)
        filterQueries.Add(new TermQuery { Field = "isActive", Value = true });
        filterQueries.Add(new DateRangeQuery { Field = "createdAt", Gte = request.StartDate });

        // 🔴 Exclusions (الأشياء التي لا نريد رؤيتها أبداً)
        foreach (var problemId in request.ExcludedProblemsIds)
        {
            mustNotQueries.Add(new TermQuery { Field = "id", Value = problemId.ToString() });
        }
        foreach (var authorId in request.ExcludedProfilesIds)
        {
            mustNotQueries.Add(new TermQuery { Field = "authorId", Value = authorId.ToString() });
        }

        // 🟡 Should (شروط ترفع التقييم Score إذا تحققت، ولكن لا تحذف المشكلة إذا لم تتحقق)

        // 1. إذا كان الكاتب من الأشخاص الذين أتابعهم (Boost قوي)
        foreach (var followedId in request.FollowedProfilesIds)
        {
            shouldQueries.Add(new TermQuery { Field = "authorId", Value = followedId.ToString() });
        }

        // 2. إذا كانت المشكلة في مجتمع أنا مشترك فيه (Boost متوسط)
        foreach (var communityId in request.CommunitiesIds)
        {
            shouldQueries.Add(new TermQuery { Field = "communityId", Value = communityId.ToString() });
        }

        // 3. مطابقة المهارات والمواضيع مع محتوى المشكلة (Soft Match)
        var userInterests = request.Skills.Concat(request.Topics).Concat(request.Tags).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        if (userInterests.Any())
        {
            shouldQueries.Add(new MultiMatchQuery
            {
                Query = string.Join(" ", userInterests),
                Fields = new[] { "title^2.0", "readmeContent", "previewBlocks.content" }
            });
        }

        // 🔥 خوارزمية الترتيب الرياضية (Function Score)
        var scoreFunctions = new List<FunctionScore>
        {
            // إعطاء وزن إضافي للمشاكل التي حصلت على أصوات (Upvotes) عالية
            new FunctionScore
            {
                FieldValueFactor = new FieldValueFactorScoreFunction
                {
                    Field = "upVotesCount",
                    Factor = 0.2, // وزن التأثير
                    Modifier = FieldValueFactorModifier.Log1p // Log1p يمنع الأرقام الفلكية من تدمير الخوارزمية
                }
            },
            // إعطاء وزن للمشاكل التي حصلت على حلول (تفاعل عالٍ)
            new FunctionScore
            {
                FieldValueFactor = new FieldValueFactorScoreFunction
                {
                    Field = "solutionsCount",
                    Factor = 0.1,
                    Modifier = FieldValueFactorModifier.Log1p
                }
            }
        };

        return await _elasticBaseService.SearchAsync(IndexName, s => s
            .Size(request.FeedSize) // لا يوجد From هنا لأن الـ Feed عادة يعمل بنظام الـ Cursor أو استدعاءات محدودة
            .Query(q => q
                .FunctionScore(fs => fs
                    .Query(qb => qb
                        .Bool(b => b
                            .Filter(filterQueries)
                            .Should(shouldQueries)
                            .MustNot(mustNotQueries)
                        )
                    )
                    .Functions(scoreFunctions)
                    .ScoreMode(FunctionScoreMode.Sum)
                    .BoostMode(FunctionBoostMode.Multiply) // نضرب الـ Score الأساسي بالـ Function Score
                )
            )
            .Sort(sort => sort.Score()), // الترتيب يتم كلياً بناءً على تقييم خوارزمية الـ Feed
            cancellationToken);
    }

    // ==========================================
    // 3. عمليات التزامن (Sync)
    // ==========================================
    public async Task<AppResult> UpsertProblemAsync(ProblemDocument problem, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.UpsertAsync(IndexName, problem.Id.ToString(), problem, cancellationToken);
    }

    public async Task<AppResult> DeleteProblemAsync(Guid problemId, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.DeleteAsync(IndexName, problemId.ToString(), cancellationToken);
    }

    public async Task<AppResult> BulkProblemsAsync(List<ProblemDocument> problems, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.BulkIndexDocumentAsync(IndexName, problems, cancellationToken);
    }

    public async Task<AppResult> DeleteProblemsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        var query = new Query
        {
            Term = new TermQuery
            {
                Field = "authorId",
                Value = authorId.ToString()
            }
        };

        return await _elasticBaseService.DeleteByQueryAsync(IndexName, 
            d => d
            .Query(q => q
                .Term(t => t
                    .Field(f => f.AuthorId)
                        .Value(authorId.ToString()))), cancellationToken);
    }
}
