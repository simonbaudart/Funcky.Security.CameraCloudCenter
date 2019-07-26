﻿import React from "react";

import {AjaxService} from "../../Services";
import {ContextAwareProps, withContext} from "../../Hoc";
import {Routes} from "../../Routing";

interface AuthenticationPanelProps extends ContextAwareProps
{
}

interface AuthenticationPanelState
{
    email?: string;
    password?: string;
    credentialsError: boolean;
    validationErrors: string[];
}

class AuthenticationPanelComponent extends React.Component<AuthenticationPanelProps, AuthenticationPanelState>
{
    constructor(props: AuthenticationPanelProps)
    {
        super(props);

        this.state = {
            credentialsError: false,
            validationErrors: []
        };
    }

    public componentDidMount()
    {
    }

    public async login()
    {
        this.setState({credentialsError: false, validationErrors: []});

        const yup = await import(/* webpackChunkName: "yup" */  'yup');

        let schema = yup.object().shape({
            login: yup.string().email().required(),
            password: yup.string().required()
        });

        const data = {
            login: this.state.email,
            password: this.state.password
        };

        schema.validate(data).then(() =>
        {
            AjaxService.postNoReturn("api/login", data).then(() =>
            {
                this.props.context.setRoute(Routes.dashboard);
            }).catch(() =>
            {
                this.setState({credentialsError: true});
            });
        }).catch((validationError) =>
        {
            this.setState({validationErrors: validationError.errors});
        });
    }

    public render()
    {
        return <>
            <div className="row">
                <div className="col-12 col-md-6 col-lg-4 mx-auto">
                    <h1>Please enter your credentials</h1>
                </div>
            </div>
            <div className="row">
                <div className="col-12 col-md-6 col-lg-4 mx-auto">
                    <form>
                        <div className="form-group">
                            <label htmlFor="email" className="bmd-label-floating">Email address</label>
                            <input type="email" className="form-control" id="email"
                                   onChange={(e) => this.setState({email: e.target.value})}/>
                        </div>
                        <div className="form-group">
                            <label htmlFor="password" className="bmd-label-floating">Password</label>
                            <input type="password" className="form-control" id="password"
                                   onChange={(e) => this.setState({password: e.target.value})}/>
                        </div>
                        <div className="form-group text-right">
                            <button type="submit" className="btn btn-primary btn-raised" onClick={async (e) =>
                            {
                                e.preventDefault();
                                await this.login();
                            }}>Submit
                            </button>
                        </div>
                        {this.getValidationMessage()}
                        {this.getErrorMessage()}
                    </form>
                </div>
            </div>
        </>;
    }

    private getErrorMessage()
    {
        let errorMessage = <></>;

        if (this.state.credentialsError)
        {
            errorMessage = <div className="alert alert-danger" role="alert">
                Your credentials are invalid, please try again.
            </div>;
        }
        
        return errorMessage;
    }

    private getValidationMessage()
    {
        let validationMessage = <></>;

        if (this.state.validationErrors && this.state.validationErrors.length > 0)
        {
            validationMessage = <div className="alert alert-danger" role="alert">
                Please check the data :
                <ul>
                    {
                        this.state.validationErrors.map((error, i) => {
                            return <li key={i}>
                                {error}
                            </li>;
                        })
                    }
                </ul>
            </div>;
        }

        return validationMessage;
    }
}

export const AuthenticationPanel = withContext<AuthenticationPanelProps>(AuthenticationPanelComponent);

export default AuthenticationPanel;