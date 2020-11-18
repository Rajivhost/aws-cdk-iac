namespace Pipeline

open Amazon.CDK
open Amazon.CDK.Pipelines
open Amazon.CDK.AWS.CodeBuild
open Amazon.CDK.AWS.CodePipeline
open Amazon.CDK.AWS.CodePipeline.Actions

type EnvStackProps =
    inherit IStackProps
    abstract Production: bool

type PipelineStack(scope, id, props: StackProps) as this =
    inherit Stack(scope, id, props)

    let sourceArtefact = Artifact_()
    let cloudAssemblyArtifact = Artifact_()

    let sourceAction =
        GitHubSourceAction
            (GitHubSourceActionProps
                (ActionName = "GitHub",
                 Output = sourceArtefact,
                 OauthToken = SecretValue.SecretsManager("github-token"),
                 Branch = "master",
                 Owner = "Rajivhost",
                 Repo = "aws-cdk-iac",
                 Trigger = GitHubTrigger.POLL))

    let synthAction =
        SimpleSynthAction
            (SimpleSynthActionProps
                (SourceArtifact = sourceArtefact,
                 CloudAssemblyArtifact = cloudAssemblyArtifact,
                 Environment = BuildEnvironment((*BuildImage = LinuxBuildImage.STANDARD_2_0,*) Privileged = true),
                 InstallCommands =
                     [| "npm install -g aws-cdk"
                        "wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb"
                        "dpkg -i packages-microsoft-prod.deb"
                        "apt-get update"
                        "apt-get install -y apt-transport-https && apt-get update && apt-get install -y dotnet-sdk-5.0" |],
                 BuildCommands = [| "dotnet build lambda-fsharp/LambdaCdk/LambdaCdk.fsproj" |],
                 SynthCommand = "cdk synth"))


    let cdkPipelineProps =
        CdkPipelineProps
            (PipelineName = "Cdk-Customers-Pipeline",
             CloudAssemblyArtifact = cloudAssemblyArtifact,
             SourceAction = sourceAction,
             SynthAction = synthAction)

    let pipeline =
        CdkPipeline(this, "Pipeline", cdkPipelineProps)
