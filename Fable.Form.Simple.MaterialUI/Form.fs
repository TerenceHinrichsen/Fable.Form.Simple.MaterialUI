namespace Fable.Form.Simple.MaterialUI

[<RequireQualifiedAccess>]
module Form =
    module View =

        open Feliz
        open Feliz.MaterialUI
        open Fable.Form.Simple.Form.View

        let fieldLabel (label: string) =
            Mui.formControlLabel [ formControlLabel.label label ]

        let errorMessage (message : string) =
            Mui.typography [
                typography.color.error
                typography.variant.body1
                typography.children message
            ]

        let wrapFieldLInContainer (children: List<ReactElement>) =
            Mui.formControl [
                prop.children children
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

            [fieldLabel config.Attributes.Label; inputControl config ]
            |> wrapFieldLInContainer


        let form (config: FormConfig<'Msg>) =
            Html.form [
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

        let textAreaField (config: TextFieldConfig<'Msg>) =
            let textField =
              Mui.textField [
                  prop.onChange (fun (text: string) -> config.OnChange text |> config.Dispatch)
                  prop.disabled config.Disabled
                  prop.value config.Value
                  prop.placeholder config.Attributes.Placeholder
                  textField.error (config.ShowError && config.Error.IsSome)

                  match config.OnBlur with
                  | Some onBlur -> prop.onBlur (fun _ -> config.Dispatch onBlur)
                  | None -> ()
              ]
            [fieldLabel config.Attributes.Label;  textField ]
            |> wrapFieldLInContainer

        let checkboxField (config : CheckboxFieldConfig<'Msg>) =
           [ Mui.checkbox [
                prop.onChange (fun (isChecked: bool) -> config.OnChange isChecked |> config.Dispatch)
                prop.disabled config.Disabled
                prop.isChecked config.Value

                match config.OnBlur with
                | Some onBlur -> prop.onBlur (fun _ -> config.Dispatch onBlur)
                | None -> ()
              ]
             fieldLabel config.Attributes.Text
           ] |> wrapFieldLInContainer

        let radioField (config: RadioFieldConfig<'Msg>) =

            let radio (key: string, label : string) =
                Mui.radio [
                    prop.name label
                    prop.isChecked (key = config.Value : bool)
                    prop.disabled config.Disabled
                    prop.onChange (fun (_: bool) -> config.OnChange key |> config.Dispatch)

                    match config.OnBlur with
                    | Some onBlur -> prop.onBlur (fun _ -> config.Dispatch onBlur)
                    | None -> ()
                ]

            [
              fieldLabel config.Attributes.Label
              Mui.radioGroup [
                radioGroup.children (config.Attributes.Options |> List.map radio)
                radioGroup.name config.Attributes.Label
                radioGroup.value config.Value ]
            ] |> wrapFieldLInContainer

        let selectField (config: SelectFieldConfig<'Msg>) =
            [
              Mui.select [
                  select.id config.Attributes.Label
                  select.value config.Value
                  select.onChange (fun newValue -> config.OnChange newValue |> config.Dispatch)

                  select.placeholder config.Attributes.Placeholder

                  match config.OnBlur with
                  | Some onBlur -> select.onBlur (fun _ -> config.Dispatch onBlur)
                  | None -> ()

                  select.children (
                      config.Attributes.Options
                      |> List.map (fun  (key: string, label: string) ->
                          Mui.menuItem [
                              menuItem.children label
                              prop.value key ]
                          )
                      )
              ]

              fieldLabel config.Attributes.Label
            ] |> wrapFieldLInContainer


        let formGroup (fields: List<ReactElement>) =
            Mui.formGroup [ formGroup.children fields ]

        let section (title: string) (fields: List<ReactElement>) =
            Mui.formGroup [
                prop.text title

                prop.children [
                    Html.legend [ prop.text title ]
                    yield! fields
                ]
            ]

        open Fable.MaterialUI.Icons

        let formList (formConfig: FormListConfig<'Msg>) =
            let addButton =
              match formConfig.Disabled, formConfig.Add with
              | false, Some add ->
                  Mui.iconButton [
                      iconButton.size.small
                      prop.onClick (fun _ -> add.Action() |> formConfig.Dispatch)
                      prop.children [
                        addIcon []
                        fieldLabel add.Label
                        ]
                  ]
              | _ -> Html.none

            Mui.container [
                fieldLabel formConfig.Label
                yield! formConfig.Forms
                addButton
            ]

        let formListItem (config: FormListItemConfig<'Msg>) =
            let deleteButton =
              match config.Disabled, config.Delete with
              | false, Some delete ->
                  Mui.iconButton [
                      iconButton.size.small
                      prop.onClick (fun _ -> delete.Action() |> config.Dispatch)
                      prop.children [
                        deleteIcon []
                        fieldLabel delete.Label
                        ]
                  ]
              | _ -> Html.none

            Mui.container [
                prop.children [
                    yield! config.Fields
                    deleteButton
                ]
            ]

        let htmlViewConfig<'Msg> : CustomConfig<'Msg> =
            {
                Form = form
                TextField = inputField Text
                PasswordField = inputField Password
                EmailField = inputField Email
                TextAreaField = textAreaField
                CheckboxField = checkboxField
                RadioField = radioField
                SelectField = selectField
                Group = formGroup
                Section = section
                FormList = formList
                FormListItem = formListItem
            }

        // Function which will be called by the consumer to render the form
        let asHtml (config : ViewConfig<'Values, 'Msg>) =
            custom htmlViewConfig config