import os
import re
import json

base_path = r"x:\Social-Networking-System\backend\SNS.Infrastructure"
config_files = []

# Find all .cs files in directories named "Configurations"
for root, dirs, files in os.walk(base_path):
    if os.path.basename(root) == "Configurations":
        for file in files:
            if file.endswith(".cs"):
                config_files.append(os.path.join(root, file))

relationships = []

for file_path in config_files:
    file_name = os.path.basename(file_path)
    with open(file_path, "r", encoding="utf-8") as f:
        content = f.read()
    
    # Extract configured entity from class declaration: class UserConfigurations : IEntityTypeConfiguration<User>
    entity_match = re.search(r"IEntityTypeConfiguration\s*<\s*([a-zA-Z0-9_\.]+)\s*>", content)
    if entity_match:
        entity = entity_match.group(1).split(".")[-1]
    else:
        entity = file_name.replace("Configurations.cs", "").replace("Configuration.cs", "")
    
    # Split content by builder calls
    # We can split on builder.HasOne or builder.HasMany
    calls = re.split(r"builder\.", content)
    
    for call in calls:
        if not (call.startswith("HasOne") or call.startswith("HasMany")):
            continue
            
        # Clean up comments and formatting
        # HasOne/HasMany signature
        has_match = re.match(r"^(HasOne|HasMany)\s*(?:<\s*([a-zA-Z0-9_\.]+)\s*>)?\s*\(\s*([a-zA-Z0-9_.\s=>\(\)]+)?\)", call, re.DOTALL)
        if not has_match:
            continue
            
        has_type = has_match.group(1)
        has_type_arg = has_match.group(2)
        has_expr_arg = has_match.group(3)
        
        target = "Unknown"
        if has_type_arg:
            target = has_type_arg.split(".")[-1]
        elif has_expr_arg:
            # e.g., ca => ca.Profile or x => x.Muted or rs => rs.Resume
            # Find the member accessed
            member_match = re.search(r"=[>]?\s*[a-zA-Z0-9_]+\.([a-zA-Z0-9_]+)", has_expr_arg)
            if member_match:
                target = member_match.group(1)
            else:
                target = has_expr_arg.strip()
                
        # Find WithOne / WithMany
        with_match = re.search(r"\.With(One|Many)\s*\(\s*([a-zA-Z0-9_.\s=>\(\)]+)?\)", call)
        with_type = ""
        with_prop = ""
        if with_match:
            with_type = with_match.group(1)
            with_expr = with_match.group(2)
            if with_expr:
                member_match = re.search(r"=[>]?\s*[a-zA-Z0-9_]+\.([a-zA-Z0-9_]+)", with_expr)
                if member_match:
                    with_prop = member_match.group(1)
                    
        # Find OnDelete
        on_delete_match = re.search(r"\.OnDelete\s*\(\s*DeleteBehavior\.([a-zA-Z0-9_]+)\s*\)", call)
        if on_delete_match:
            on_delete = on_delete_match.group(1)
        else:
            on_delete = "Implicit (Default)"
            
        # Find IsRequired
        is_required = "No"
        if re.search(r"\.IsRequired\s*\(\s*(?:true)?\s*\)", call):
            is_required = "Yes"
            
        relationships.append({
            "File": file_name,
            "SourceEntity": entity,
            "HasType": has_type,
            "Target": target,
            "WithType": with_type,
            "WithProp": with_prop,
            "OnDelete": on_delete,
            "IsRequired": is_required,
            "RawCall": "builder." + call[:200].replace("\n", " ").strip()
        })

print(json.dumps(relationships, indent=2))
with open(r"x:\Social-Networking-System\backend\parsed_py.json", "w", encoding="utf-8") as f:
    json.dump(relationships, f, indent=2)
