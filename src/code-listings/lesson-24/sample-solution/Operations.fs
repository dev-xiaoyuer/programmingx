module Capstone4.Operations

open System
open Capstone4.Domain

let private rateAccount account =
    if account.Balance >= 0M then (InCredit(CreditAccount account))
    else Overdrawn account

/// Withdraws an amount of an account (if there are sufficient funds)
let withdraw amount (CreditAccount account) =
    { account with Balance = account.Balance - amount }
    |> rateAccount

/// Deposits an amount into an account
let deposit amount account =
    let account =
        match account with
        | Overdrawn account -> account
        | InCredit (CreditAccount account) -> account
    { account with Balance = account.Balance + amount }
    |> rateAccount

/// Runs some account operation such as withdraw or deposit with auditing.
let auditAs operationName audit operation amount account accountId owner =
    let updatedAccount = operation amount account
    let transaction = { Operation = operationName; Amount = amount; Timestamp = DateTime.UtcNow }

    audit accountId owner.Name transaction
    updatedAccount

let tryParseSerializedOperation operation =
    match operation with
    | "withdraw" -> Some Withdraw
    | "deposit" -> Some Deposit
    | _ -> None

/// Creates an account from a historical set of transactions
let loadAccount (owner, accountId, transactions) =
    let openingAccount = Overdrawn { AccountId = accountId; Balance = 0M; Owner = { Name = owner } }

    transactions
    |> Seq.sortBy(fun txn -> txn.Timestamp)
    |> Seq.fold(fun account txn ->
        match tryParseSerializedOperation txn.Operation with
        | Some Deposit -> account |> deposit txn.Amount
        | Some Withdraw ->
            match account with
            | InCredit account -> account |> withdraw txn.Amount
            | account -> account
        | None -> account) openingAccount