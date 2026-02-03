# CI/CD Pipeline Documentation

## Pipeline Overview

Our GitHub Actions pipeline provides automated building, testing, and quality assurance for the Contract Plan Manager project.

## Workflows

### Main CI/CD Pipeline (`ci-cd.yml`)

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main`

**Jobs:**

#### 1. Build and Test
- Sets up .NET 10.0
- Starts SQL Server service for integration tests
- Restores dependencies
- Builds solution in Release mode
- Runs unit tests
- Runs integration tests
- Generates test reports
- Collects code coverage
- Uploads coverage to Codecov

#### 2. Code Quality Analysis
- Runs `dotnet format` to check code formatting
- Performs security scanning with DevSkim
- Uploads security findings to GitHub Security tab

#### 3. Docker Build
- Builds Docker image for the API
- Uses layer caching for faster builds
- Validates the image starts correctly
- Only runs after tests pass

#### 4. Dependency Review
- Reviews dependency changes in pull requests
- Alerts on vulnerable or incompatible dependencies
- Only runs on PRs

#### 5. Release Notes
- Generates changelog from commit messages
- Creates release notes artifact
- Only runs on main branch pushes

## Status Badges

Add these to your main README.md:
```markdown
![Build Status](https://github.com/<username>/ContractPlanManager/workflows/CI-CD%20Pipeline/badge.svg)
![Code Coverage](https://codecov.io/gh/<username>/ContractPlanManager/branch/main/graph/badge.svg)
```

## Local Testing

To run the same checks locally before pushing:
```bash
# Restore and build
dotnet restore
dotnet build --configuration Release

# Run tests
dotnet test --configuration Release

# Check formatting
dotnet format --verify-no-changes

# Build Docker image
docker build -f src/API/Dockerfile -t contract-api:local .
```

## Best Practices

1. **Commit Messages**: Use conventional commit format:
   - `feat: add new feature`
   - `fix: resolve bug`
   - `test: add tests`
   - `docs: update documentation`
   - `chore: maintenance tasks`

2. **Pull Requests**: 
   - All checks must pass before merging
   - Code coverage should not decrease
   - No security vulnerabilities introduced

3. **Branches**:
   - `main`: Production-ready code
   - `develop`: Integration branch
   - `feature/*`: New features
   - `fix/*`: Bug fixes

## Secrets Configuration

For full functionality, configure these secrets in GitHub repository settings:

- `CODECOV_TOKEN`: For code coverage reporting (optional)
- Future: Add Docker Hub credentials for image publishing
- Future: Add cloud provider credentials for deployment

## Troubleshooting

### Tests Failing in CI but Passing Locally
- Check SQL Server connection string in workflow
- Verify service containers are healthy
- Review workflow logs for specific errors

### Docker Build Failures
- Ensure Dockerfile is in correct location
- Check that all project references are correct
- Verify base images are accessible

### Code Coverage Not Uploading
- Ensure coverlet.collector is installed
- Check Codecov token is configured
- Verify coverage files are generated

## Future Enhancements

- [ ] Deploy to staging environment on develop branch
- [ ] Deploy to production on main branch (with approval)
- [ ] Performance testing integration
- [ ] Automated security scanning with additional tools
- [ ] Database migration verification
- [ ] Container vulnerability scanning