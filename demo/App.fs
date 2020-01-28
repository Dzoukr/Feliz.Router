module App

open Elmish
open Elmish.React
open Feliz
open Feliz.Router

type State = { CurrentUrl : string list }

type Msg =
    | UrlChanged of string list
    | NavigateUsers
    | NavigateToUser of int
    | NavigateToUserReplaceState of int

let init() = { CurrentUrl = Router.currentUrl() }, Cmd.none

let commands() = [
    Router.navigate("one", "two", "three")
    Router.navigate("user", 15)
    Router.navigate("one", "two", [ "limit", "10"; "id", "20" ])
]

let update msg state =
    match msg with
    | UrlChanged segments -> { state with CurrentUrl = segments }, Cmd.none
    | NavigateUsers -> state, Router.navigate("users")
    | NavigateToUser userId -> state, Router.navigate("users", [ "id", userId ])
    | NavigateToUserReplaceState userId -> state, Router.navigate("users", [ "id", userId ], HistoryMode.ReplaceState)

let render state dispatch =
    let currentPage =
        match state.CurrentUrl with
        | [ ] ->
            Html.div [
                Html.button [
                    prop.text "Users"
                    prop.onClick (fun _ -> dispatch NavigateUsers)
                ]
                Html.a [
                    prop.href (Router.format("users"))
                    prop.text "Users link"
                ]
            ]

        | [ "users" ] ->
            Html.div [
                Html.button [
                    prop.text "Single User (History.PushState)"
                    prop.onClick (fun _ -> dispatch (NavigateToUser 10))
                ]

                Html.button [
                    prop.text "Single User (History.ReplaceState)"
                    prop.onClick (fun _ -> dispatch (NavigateToUserReplaceState 10))
                ]

                Html.a [
                    prop.href (Router.format("users", ["id", 10]))
                    prop.text "Single User link"
                ]
            ]

        | [ "users"; Route.Int userId ] ->
            Html.h1 (sprintf "User ID %d" userId)

        | [ "users"; Route.Query [ "id", Route.Int userId ] ] ->
            Html.h1 (sprintf "Query String => User ID %d" userId)

        | [ "users"; Route.Query [ "search", username ] ] ->
            Html.h1 (sprintf "Query string => Username %s" username)

        | _ ->
            Html.h1 "Not Found"

    Router.router [
        Router.hashMode
        Router.onUrlChanged (UrlChanged >> dispatch)
        Router.application [
            Html.div [
                prop.style [ style.padding 20 ]
                prop.children [
                    currentPage
                ]
            ]
        ]
    ]

Program.mkProgram init update render
|> Program.withReactSynchronous "root"
|> Program.withConsoleTrace
|> Program.run