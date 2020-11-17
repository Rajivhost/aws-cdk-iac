namespace App

open Amazon.CDK
open Amazon.CDK.AWS.DynamoDB

type EnvStackProps =
    inherit IStackProps
    abstract Production: bool

//type AppStack(scope, id, props: EnvStackProps) as this =
type AppStack(scope, id, props: StackProps, isProd: bool) as this =
    inherit Stack(scope, id, props)

    // The code that defines your stack goes here
    // Defining the prod or no prod

    let stackParams =
        match isProd with
        | true ->
            {| DynamoDbReadWrite = 200.
               TableName = "Cdk-Customers-Production" |}
        | false ->
            {| DynamoDbReadWrite = 5.
               TableName = "Cdk-Customers-Staging" |}

        //match props with
        //| props when props.Production ->
        //    {| DynamoDbReadWrite = 200.
        //       TableName = "Cdk-Customers-Production" |}
        //| props when props.Production |> not ->
        //    {| DynamoDbReadWrite = 5.
        //       TableName = "Cdk-Customers-Staging" |}

    // --- dynamodb ---
    let table =
        let tableProps =
            TableProps
                (TableName = stackParams.TableName,
                 ReadCapacity = stackParams.DynamoDbReadWrite,
                 PartitionKey = new Attribute(Type = AttributeType.STRING, Name = "PK"),
                 BillingMode = BillingMode.PROVISIONED)
        Table(this, "customers", tableProps)
