namespace Fable.Form.Simple.MaterialUI

[<RequireQualifiedAccess>]
module Form =
    module View =

        open Fable.Form
        open Feliz
        open Feliz.MaterialUI
        open Fable.Form.Simple
        open Fable.Form.Simple.Form.View

        let fieldLabel (label: string) =
            Mui.typography [
                typography.variant.caption
                typography.children label
            ]

        let errorMessage (message : string) =
            Mui.typography [
                typography.color.error
                typography.variant.body1
                typography.children message
            ]

        let inputFieldBase type' (config: TextFieldConfig<'Msg>) =
              Mui.input [
                  input.type' type'
                  prop.onChange (fun (text : string) -> config.OnChange text |> config.Dispatch)

                  match config.OnBlur with
                  | Some onBlur -> prop.onBlur (fun _ -> config.Dispatch onBlur )
                  | None -> ()

                  prop.disabled config.Disabled
                  prop.value config.Value
                  prop.placeholder config.Attributes.Placeholder
                  input.error (config.ShowError && config.Error.IsSome)
              ]

        let inputField
            (typ : InputType)
            (config: TextFieldConfig<'Msg> ) =

            let inputControl  =
                match typ with
                | InputType.Text -> inputFieldBase "Text"
                | InputType.Password -> inputFieldBase "Password"
                | InputType.Email -> inputFieldBase "Email"
            inputControl config


        let form (config: FormConfig<'Msg>) =
            Mui.formControl [

                prop.onSubmit (fun  ev ->

                    ev.stopPropagation()
                    ev.preventDefault()

                    config.OnSubmit
                    |> Option.map config.Dispatch
                    |> Option.defaultWith ignore
                    )

                prop.children [
                    yield! config.Fields

                    match config.State with
                    | Error error -> errorMessage error
                    | Success success ->
                        Mui.alert [
                            alert.color.success
                            alert.component' success
                        ]
                    | Loading
                    | Idle -> Html.none

                    Mui.divider [
                        prop.children [
                            Mui.button [
                                button.color.primary
                                button.children [
                                    if config.State = Loading then Mui.circularProgress[]
                                    fieldLabel config.Action
                                    ]
                            ]
                        ]
                    ]
                ]
            ]

        let htmlViewConfig<'Msg> : CustomConfig<'Msg> =
            {
                Form = form
                TextField = inputField Text
                PasswordField = inputField Password
                EmailField = inputField Email
                TextAreaField = failwith "Not implemented yet"
                CheckboxField = failwith "Not implemented yet"
                RadioField = failwith "Not implemented yet"
                SelectField = failwith "Not implemented yet"
                Group = failwith "Not implemented yet"
                Section = failwith "Not implemented yet"
                FormList = failwith "Not implemented yet"
                FormListItem = failwith "Not implemented yet"
            }

        // Function which will be called by the consumer to render the form
        let asHtml (config : ViewConfig<'Values, 'Msg>) =
            custom htmlViewConfig config