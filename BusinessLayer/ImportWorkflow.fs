module ImportWorkflow

open StateChecker

let importCustomers loadCustomersFunc mappingFunc saveCustomersFunc updateInputStatusFunc loggingFunc =
    //todo: convert all Failures to NewFailures
    loadCustomersFunc()
    |> checkState mappingFunc
    |> checkState saveCustomersFunc
    |> updateInputStatusFunc //the last 2 steps handle both success and failure, so we don't call checkState on them
    |> loggingFunc
    