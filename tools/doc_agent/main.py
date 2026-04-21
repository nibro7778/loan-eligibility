from pathlib import Path
import subprocess
from openai import OpenAI

client = OpenAI()


# ----------------------------
# Load agent instructions
# ----------------------------
def load_agent_prompt():
    path = Path(".github/agents/doc-agent.md")
    return path.read_text(encoding="utf-8")


# ----------------------------
# Multi-commit diff (CORE UPGRADE)
# ----------------------------
def get_diff():
    # Ensure we have full history
    subprocess.run(["git", "fetch", "origin", "main"], check=True)

    diff = subprocess.check_output(
        ["git", "diff", "origin/main...HEAD"],
        text=True
    )

    return diff


# ----------------------------
# Commit context (intent layer)
# ----------------------------
def get_commit_log():
    return subprocess.check_output(
        ["git", "log", "origin/main..HEAD", "--oneline"],
        text=True
    )


# ----------------------------
# Read README
# ----------------------------
def read_readme():
    return Path("README.md").read_text(encoding="utf-8")


# ----------------------------
# Build intelligent prompt
# ----------------------------
def build_prompt(diff, commits, readme):
    return f"""
You are a senior software documentation engineer.

You are analyzing a MULTI-COMMIT CHANGE SET.

========================
COMMITS (intent layer)
========================
{commits}

========================
CODE DIFF (source of truth)
========================
{diff}

========================
CURRENT README
========================
{readme}

========================
INSTRUCTIONS
========================
- Treat all commits as ONE logical feature/change
- Do NOT document commit-by-commit changes
- Understand final system behavior only
- Update README.md accordingly
- Remove outdated or incorrect documentation
- Keep structure clean and consistent
- Do NOT hallucinate features not in diff
- Return FULL updated README.md ONLY
"""


# ----------------------------
# Generate updated README
# ----------------------------
def generate_updated_readme(diff, commits, readme):
    system_prompt = load_agent_prompt()
    user_prompt = build_prompt(diff, commits, readme)

    response = client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": user_prompt}
        ],
        temperature=0.2
    )

    return response.choices[0].message.content


# ----------------------------
# Main execution
# ----------------------------
def main():
    print("🚀 Doc Agent started")

    # Ensure full git context
    subprocess.run(["git", "fetch", "origin", "main"], check=True)

    diff = get_diff()
    commits = get_commit_log()
    readme = read_readme()

    print(f"📊 Commits detected:\n{commits}")
    print(f"📏 Diff size: {len(diff)} characters")

    if not diff.strip():
        print("⚠️ No changes detected. Exiting.")
        return

    updated_readme = generate_updated_readme(diff, commits, readme)

    Path("README.md").write_text(updated_readme, encoding="utf-8")

    print("✅ README updated successfully")


if __name__ == "__main__":
    main()