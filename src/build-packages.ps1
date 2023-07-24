Remove-Item ./packages/* -Recurse

dotnet pack ./Thinktecture.SemanticKernel.sln --configuration Release --output ./packages /p:Deterministic=true /p:ContinuousIntegrationBuild=true