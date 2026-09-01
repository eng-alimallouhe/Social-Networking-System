export const DISCUSSIONS_API_ROUTES = {
    // Problems
    Problems: 'problems',
    ProblemById: (problemId: string) => `problems/${problemId}`,
    MyProblems: 'problems/my-problems',
    ProblemsByAuthor: (authorId: string) => `problems/author/${authorId}`,
    ProblemsByCommunity: (communityId: string) => `problems/community/${communityId}`,
    ChangeProblemStatus: (problemId: string) => `problems/${problemId}/status`,

    // Problem Votes
    ProblemVotes: (problemId: string) => `problems/${problemId}/votes`,
    ProblemVoteSummary: (problemId: string) => `problems/${problemId}/votes/summary`,
    MyProblemVote: (problemId: string) => `problems/${problemId}/votes/my-vote`,

    // Problem Views
    ProblemViews: (problemId: string) => `problems/${problemId}/views`,
    ProblemViewers: (problemId: string) => `problems/${problemId}/views/viewers`,

    // Problem Tags
    ProblemTags: (problemId: string) => `problems/${problemId}/tags`,
    RemoveProblemTag: (problemId: string, tagId: string) => `problems/${problemId}/tags/${tagId}`,

    // Problem Topics
    ProblemTopics: (problemId: string) => `problems/${problemId}/topics`,

    // Solutions
    Solutions: 'solutions',
    SolutionById: (solutionId: string) => `solutions/${solutionId}`,
    MySolutions: 'solutions/my-solutions',
    SolutionsByAuthor: (authorId: string) => `solutions/author/${authorId}`,
    ProblemSolutions: (problemId: string) => `problems/${problemId}/solutions`,
    ChangeSolutionStatus: (solutionId: string) => `solutions/${solutionId}/status`,

    // Solution Votes
    SolutionVotes: (solutionId: string) => `solutions/${solutionId}/votes`,
    SolutionVoteSummary: (solutionId: string) => `solutions/${solutionId}/votes/summary`,
    MySolutionVote: (solutionId: string) => `solutions/${solutionId}/votes/my-vote`,
} as const;
