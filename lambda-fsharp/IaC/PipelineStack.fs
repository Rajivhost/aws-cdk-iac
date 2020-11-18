namespace Pipeline

open Amazon.CDK
open Amazon.CDK.Pipelines
open Amazon.CDK.AWS.CodePipeline
open Amazon.CDK.AWS.CodePipeline.Actions

type EnvStackProps =
    inherit IStackProps
    abstract Production: bool

type PipelineStack(scope, id, props: StackProps) as this =
    inherit Stack(scope, id, props)

    // --- repository ---
    let sourceArtefact = Artifact_()
    let cloudAssemblyArtifact = Artifact_()

    let cdkPipelineProps =
        CdkPipelineProps
            (CloudAssemblyArtifact = cloudAssemblyArtifact,
             PipelineName = "Cdk-Customers-Pipeline",
             SourceAction =
                 GitHubSourceAction
                     (GitHubSourceActionProps
                         (ActionName = "GitHub",
                          Output = sourceArtefact,
                          OauthToken = SecretValue.SecretsManager("github-token"),
                          Owner = "Rajivhost",
                          Repo = "aws-cdk-iac",
                          Trigger = GitHubTrigger.POLL)),
             SynthAction =
                 SimpleSynthAction
                     (SimpleSynthActionProps
                         (SourceArtifact = sourceArtefact,
                          CloudAssemblyArtifact = cloudAssemblyArtifact,
                          InstallCommands =
                              [| "npm install -g aws-cdk"
                                 "npm install -g dotnet-sdk-3.1"
                                 "dotnet restore" |],
                          BuildCommands = [|"dotnet build"|],
                          SynthCommand = "cdk synth"))

            )

    let pipeline =
        CdkPipeline(this, "Pipeline", cdkPipelineProps)
