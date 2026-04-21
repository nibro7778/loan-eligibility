from pathlib import Path
import subprocess
import json
from openai import OpenAI

client = OpenAI()

# ----------------------------
# State file (commit tracking)
# ----------------------------
STATE_FILE = Path(".github/doc-state.json")


# ----------------------------
# Load agent instructions
# ----------------------------
def load_agent_prompt():
    return Path(".github/agents/doc-agent.md").read_text(encoding="utf-8")


# ----------------------------
# Get current commit
# ----------------------------
def get_current_commit():
    return subprocess.check_output(
        ["git", "rev-parse", "HEAD"],
        text=True
    ).strip()


# ----------------------------
# Load last processed commit
# (auto-create if missing)
# ----------------------------
def get_last_commit():
    if not STATE_FILE.exists():
        print("🆕 No state file found. Creating new one.")
        return None

    try:
        data = json.loads(STATE_FILE.read_text())
        return data.get("lastCommit")
    except Exception:
        print("⚠️ Corrupted state file. Resetting.")
        return None


# ----------------------------
# Save last processed commit
# ----------------------------
def save_last_commit(commit):
    STATE_FILE.parent.mkdir(parents=True, exist_ok=True)

    STATE_FILE.write_text(
        json.dumps({"lastCommit": commit}, indent=2),
        encoding="utf-8"
    )


# ----------------------------
# Multi-commit diff logic
# ----------------------------
def get_diff(last_commit):
    subprocess.run(["git", "fetch", "origin", "main"], check=True)

    # First run → fallback to previous commit
    if not last_commit:
        print("⚠️ First run detected, using HEAD~1 diff")
        return subprocess.check_output(
            ["git", "diff", "HEAD~1..HEAD"],
            text=True
        )

    print(f"🔁 Diffing from last commit: {last_commit}")

    return subprocess.check_output(
        ["git", "diff", f"{last_commit}..HEAD"],
        text=True
    )


# ----------------------------
# Read README
# ----------------------------
def read_readme():
    return Path("README.md").read_text(encoding="utf-8")


# ----------------------------
# Build prompt
# ----------------------------
def build_prompt(diff, commits, readme):
    return f"""
You are a senior software documentation engineer.

You are analyzing a MULTI-COMMIT CHANGE SET.

====================
COMMITS
====================
{commits}

====================
CODE DIFF
====================
{diff}

====================
CURRENT README
====================
{readme}

====================
RULES
====================
- Treat all commits as ONE feature change
- Do NOT describe commits individually
- Update README to reflect FINAL system state
- Remove outdated information
- Keep structure clean
- Return FULL updated README only
"""


# ----------------------------
# Get commit log (intent layer)
# ----------------------------
def get_commit_log(last_commit):
    if not last_commit:
        return "Initial run (no previous commits)"

    return subprocess.check_output(
        ["git", "log", f"{last_commit}..HEAD", "--oneline"],
        text=True
    )


# ----------------------------
# Generate README
# ----------------------------
def generate_readme(diff, commits, readme):
    system_prompt = load_agent_prompt()

    response = client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[
            {"role": "system", "content": system_prompt},
            {
                "role": "user",
                "content": build_prompt(diff, commits, readme)
            }
        ],
        temperature=0.2
    )

    return response.choices[0].message.content


# ----------------------------
# MAIN
# ----------------------------
def main():
    print("🚀 Doc Agent started")

    subprocess.run(["git", "fetch", "origin", "main"], check=True)

    current_commit = get_current_commit()
    last_commit = get_last_commit()

    print(f"📌 Last commit: {last_commit}")
    print(f"📌 Current commit: {current_commit}")

    diff = get_diff(last_commit)
    commits = get_commit_log(last_commit)
    readme = read_readme()

    print(f"📏 Diff size: {len(diff)} characters")

    if not diff.strip():
        print("⚠️ No changes detected. Exiting.")
        return

    updated_readme = generate_readme(diff, commits, readme)

    Path("README.md").write_text(updated_readme, encoding="utf-8")

    # 🔥 Update state AFTER successful generation
    save_last_commit(current_commit)

    print("✅ README updated successfully")
    print("💾 State saved for next run")


if __name__ == "__main__":
    main()