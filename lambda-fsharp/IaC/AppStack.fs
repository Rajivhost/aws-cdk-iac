namespace App

open Amazon.CDK
open Amazon.CDK.AWS.Lambda
open Amazon.CDK.AWS.DynamoDB
open Amazon.CDK.AWS.APIGateway

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
               TableName = "Cdk-Customers-Production"
               ApiGatewayName = "PROD_cdk_api" |}
        | false ->
            {| DynamoDbReadWrite = 5.
               TableName = "Cdk-Customers-Staging"
               ApiGatewayName = "STAGING_cdk_api" |}

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
                 PartitionKey = new Attribute(Name = "PK", Type = AttributeType.STRING),
                 SortKey = new Attribute(Name = "SK", Type = AttributeType.STRING),
                 Stream = StreamViewType.NEW_IMAGE,
                 RemovalPolicy = RemovalPolicy.DESTROY,
                 BillingMode = BillingMode.PROVISIONED)

        Table(this, "customers", tableProps)

    // --- lambda ---
    let lambdaFunction =
        let functionProps =
            FunctionProps
                (FunctionName = "cdk-customers-service",
                 Runtime = Runtime.DOTNET_CORE_3_1,
                 Code = Code.FromAsset("lambda-fsharp/LambdaCdk/bin/Release/netcoreapp3.1/publish"),
                 Handler = "LambdaCdk::Setup+LambdaEntryPoint::FunctionHandlerAsync",
                 Timeout = Duration.Seconds(31.),
                 MemorySize = 512.)

        Function(this, "Cdk-Customers-Service", functionProps).AddEnvironment("TABLE_NAME", table.TableName)



    // --- api gateway ---
    let api = RestApi(this, stackParams.ApiGatewayName)

    // --- api gw integration ---
    let apiLambdaInteg = LambdaIntegration(lambdaFunction)
    let _ = api.Root.AddMethod("ANY", apiLambdaInteg)

    // --- table permissions ---
    let _ = table.GrantReadWriteData(lambdaFunction)
