open Amazon.CDK
open App

[<EntryPoint>]
let main _ =
    let app = App(null)

    AppStack(app, "AppStack", StackProps()) |> ignore

    app.Synth() |> ignore
    0
