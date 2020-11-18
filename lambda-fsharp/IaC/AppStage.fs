namespace App

open System
open Amazon.CDK

type AppStage(scope, id, props: StageProps) as this =
    inherit Stage(scope, id, props)

    let app =
        AppStack(this, id, StackProps(Env = props.Env), true)

    member _.ApiGatewayUrl = app.ApiGatewayUrl