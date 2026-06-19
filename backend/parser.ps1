$files = Get-ChildItem -Path x:\Social-Networking-System\backend\SNS.Infrastructure -Filter *Configurations.cs -Recurse

$results = @()

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    
    $entity = ""
    if ($content -match 'IEntityTypeConfiguration<([A-Za-z0-9_]+)>') {
        $entity = $matches[1]
    } else {
        continue
    }

    $parts = $file.FullName -split '\\'
    $idx = [array]::IndexOf($parts, "SNS.Infrastructure")
    $context = $parts[$idx + 1]

    $minified = $content -replace '\s+', ' '

    # Find HasOne patterns
    $matchesHasOne = [regex]::Matches($minified, 'HasOne(?:<([^>]+)>|\([^\)]+\))(.*?)(?:;|$)')
    
    foreach ($m in $matchesHasOne) {
        $principal = $m.Groups[1].Value
        $rest = $m.Groups[2].Value

        if ([string]::IsNullOrWhiteSpace($principal)) {
            # Try to guess from Property name if it was HasOne(x => x.UserProfile)
            if ($m.Value -match 'HasOne\([^=]+=>\s*[^.]+\.([A-Za-z0-9_]+)\)') {
                $principal = "Property:" + $matches[1]
            } else {
                $principal = "Unknown"
            }
        }
        
        $behavior = ""
        if ($rest -match 'OnDelete\(\s*DeleteBehavior\.([A-Za-z]+)\s*\)') {
            $behavior = $matches[1]
        } else {
            if ($rest -match 'IsRequired\(\s*false\s*\)') {
                $behavior = "SetNull / Restrict (Default)"
            } else {
                $behavior = "Cascade (Default)"
            }
        }

        $results += [PSCustomObject]@{
            Context = $context
            Principal = $principal
            Dependent = $entity
            Behavior = $behavior
            File = $file.Name
            Type = "HasOne"
        }
    }
    
    # Find HasMany patterns
    $matchesHasMany = [regex]::Matches($minified, 'HasMany(?:<([^>]+)>|\([^\)]+\))(.*?)(?:;|$)')
    foreach ($m in $matchesHasMany) {
        $dependent = $m.Groups[1].Value
        $rest = $m.Groups[2].Value

        if ([string]::IsNullOrWhiteSpace($dependent)) {
            if ($m.Value -match 'HasMany\([^=]+=>\s*[^.]+\.([A-Za-z0-9_]+)\)') {
                $dependent = "Property:" + $matches[1]
            } else {
                $dependent = "Unknown"
            }
        }
        
        $behavior = ""
        if ($rest -match 'OnDelete\(\s*DeleteBehavior\.([A-Za-z]+)\s*\)') {
            $behavior = $matches[1]
        } else {
            if ($rest -match 'IsRequired\(\s*false\s*\)') {
                $behavior = "SetNull / Restrict (Default)"
            } else {
                $behavior = "Cascade (Default)"
            }
        }

        $results += [PSCustomObject]@{
            Context = $context
            Principal = $entity
            Dependent = $dependent
            Behavior = $behavior
            File = $file.Name
            Type = "HasMany"
        }
    }
}

$results | ConvertTo-Json -Depth 3 | Out-File x:\Social-Networking-System\backend\relations_parsed.json
