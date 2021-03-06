﻿module ImportWorkflow

open StateChecker
open InputServiceFactory
open OutputServiceFactory

let importCustomers inputSvcCtx mappingFunc outputSvcCtx loggingFunc =
    //todo: convert all Failures to NewFailures (and all Successes to NewSuccesses)
    //todo: create another workflow that will select and log the case of a missing CustomerCompany record
    inputSvcCtx.loadCustomer()
    |> checkState mappingFunc
    |> checkState outputSvcCtx.saveCustomer
    |> checkEmptyState inputSvcCtx.updateInputStatus //the last 2 steps handle both success and failure, so we don't call checkState on them
    |> loggingFunc
    