open App
open Pipeline
open Amazon.CDK

[<EntryPoint>]
let main _ =
    let app = App(null)

    let env = Environment(Region = "eu-west-2")

    AppStack(app, "cdk-customers-prod", StackProps(Env = env), true)
    |> ignore

    AppStack(app, "cdk-customers-staging", StackProps(Env = env), false)
    |> ignore

    PipelineStack(app, "cdk-customers-pipeline", StackProps(Env = env))
    |> ignore

    //AppStack
    //    (app,
    //     "cdk-customers-prod",
    //     { new EnvStackProps with
    //         member _.Production = true

    //         member _.Env =
    //             { new IEnvironment with
    //                 member _.Region = "eu-west-2" } })
    //|> ignore

    //AppStack
    //    (app,
    //     "cdk-customers-staging",
    //     { new EnvStackProps with
    //         member _.Production = false

    //         member _.Env =
    //             { new IEnvironment with
    //                 member _.Region = "eu-west-2" } })
    //|> ignore

    app.Synth() |> ignore
    0
