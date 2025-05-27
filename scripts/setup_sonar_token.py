import time
import requests
import os
import subprocess

# Configuration
SONARQUBE_URL = "http://tc-cloudgames-sonarqube:9000"
ADMIN_CREDENTIALS = ("admin", "6283Xf*&hb@d")  # Updated password
PROJECT_KEY = "tc-cloudgames-local"
PROJECT_NAME = "TCCloudGames"
TOKEN_NAME = "tc-cloudgames-token"
TOKEN_FILE = "./token_data/sonar-token.txt"
CODE_COVERAGE_SCRIPT = "/scripts/code_coverage.sh"

def wait_for_sonarqube():
    """Waits until SonarQube is in GREEN health status."""
    print("Waiting for SonarQube to be ready...")
    while True:
        try:
            response = requests.get(f"{SONARQUBE_URL}/api/system/health", auth=ADMIN_CREDENTIALS, timeout=5)
            response.raise_for_status()
            data = response.json()
            health_status = data.get("health")

            if health_status == "GREEN":
                print("SonarQube is ready!")
                break
            else:
                print(f"Current health status: {health_status} - Waiting...")
        except requests.RequestException as e:
            print(f"Error connecting to SonarQube: {e}")

        time.sleep(5)

def check_project_exists():
    """Checks if the project already exists in SonarQube."""
    response = requests.get(f"{SONARQUBE_URL}/api/projects/search", auth=ADMIN_CREDENTIALS)
    response.raise_for_status()  # Ensure API call was successful
    projects = response.json().get("components", [])
    return any(project["key"] == PROJECT_KEY for project in projects)

def create_project():
    """Creates the project in SonarQube."""
    print("Creating project...")
    response = requests.post(
        f"{SONARQUBE_URL}/api/projects/create",
        auth=ADMIN_CREDENTIALS,
        data={"name": PROJECT_NAME, "project": PROJECT_KEY}
    )
    response.raise_for_status()  # Ensure API call was successful
    print("Project created successfully!")

def generate_token():
    """Generates a token and saves it to a file."""
    print("Generating token...")
    response = requests.post(
        f"{SONARQUBE_URL}/api/user_tokens/generate",
        auth=ADMIN_CREDENTIALS,
        data={"name": TOKEN_NAME}
    )
    response.raise_for_status()  # Ensure API call was successful
    token = response.json().get("token")

    if token:
        with open(TOKEN_FILE, "w") as f:
            f.write(token)
        print(f"Token generated and saved: {token}")
    else:
        print("Token generation failed or token already exists.")

def run_code_coverage():
    """Runs the code coverage script if both token file and script exist."""
    if os.path.exists(TOKEN_FILE):
        print("Token file exists, proceeding to code coverage execution...")
        if os.path.exists(CODE_COVERAGE_SCRIPT):
            print(f"Executing code coverage script: {CODE_COVERAGE_SCRIPT}")
            subprocess.run(["sh", CODE_COVERAGE_SCRIPT], check=True)
        else:
            print(f"ERROR: Code coverage script not found at {CODE_COVERAGE_SCRIPT}. Aborting execution.")
    else:
        print(f"ERROR: Token file not found at {TOKEN_FILE}. Aborting code coverage execution.")

if __name__ == "__main__":
    wait_for_sonarqube()

    if check_project_exists():
        print(f"Project {PROJECT_KEY} already exists.")
    else:
        create_project()
        generate_token()
    
    run_code_coverage()

    print("Setup complete.")
