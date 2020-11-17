namespace App

open Amazon.CDK

type AppStack(scope, id, props) as this =
    inherit Stack(scope, id, props)

    // The code that defines your stack goes here
