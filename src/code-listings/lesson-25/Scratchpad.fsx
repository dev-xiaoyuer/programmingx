﻿// Listing 25.3
#I @"CSharpProject\bin\debug"
#r @"CSharpProject.dll"
// Or all in one line: #r @"CSharpProject\bin\debug\CSharpProject.dll"

open CSharpProject
let simon = Person "Simon"
simon.PrintName()

// Listing 25.4
let people =
    [ "Tony"; "Fred"; "Samantha"; "Brad"; "Sophie "]
    |> List.map Person

// Listing 25.5
open System.Collections.Generic

type PersonComparer() =
   interface IComparer<Person> with
       member this.Compare(first, second) =
           first.Name.CompareTo(second.Name)

let pComparer = PersonComparer() :> IComparer<Person>
pComparer.Compare(simon, Person "Fred")

// Object expressions
let personComparer =
    { new IComparer<Person> with
          member __.Compare(x, y) = x.Name.CompareTo(y.Name) }

personComparer.Compare(simon, Person "Tony")

open System

let blank:string = null
let name = "Vera"
let number = Nullable 10

let blankAsOption = blank |> Option.ofObj
let nameAsOption = name |> Option.ofObj
let numberAsOption = number |> Option.ofNullable

let unsafeName = Some "Fred" |> Option.toObj
