from pathlib import Path
import subprocess
from openai import OpenAI

client = OpenAI()

def load_agent_prompt():
    path = Path(".github/agents/doc-agent.md")
    return path.read_text(encoding="utf-8")

def get_diff():
    try:
        return subprocess.check_output(
            ["git", "diff", "HEAD~1", "HEAD"],
            text=True
        )
    except:
        return ""

def read_readme():
    try:
        return Path("README.md").read_text(encoding="utf-8")
    except:
        return ""

def generate_updated_readme(diff, readme):
    system_prompt = load_agent_prompt()

    user_prompt = f"""
CURRENT README:
{readme}

GIT DIFF:
{diff}
"""

    response = client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": user_prompt}
        ],
        temperature=0.2
    )

    return response.choices[0].message.content