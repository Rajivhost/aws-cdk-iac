open Amazon.CDK
open App

[<EntryPoint>]
let main _ =
    let app = App(null)

    AppStack
        (app,
         "cdk-customers-prod",
         { new EnvStackProps with
             member _.Production = true

             member _.Env =
                 { new IEnvironment with
                     member _.Region = "eu-west-2" } })
    |> ignore

    AppStack
        (app,
         "cdk-customers-staging",
         { new EnvStackProps with
             member _.Production = false

             member _.Env =
                 { new IEnvironment with
                     member _.Region = "eu-west-2" } })
    |> ignore

    app.Synth() |> ignore
    0
