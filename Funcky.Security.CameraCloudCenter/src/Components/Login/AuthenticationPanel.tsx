import React from "react";

import { AjaxService } from "../../Services";
import { ContextAwareProps, withContext } from "../../Hoc";
import { Routes } from "../../Routing";

interface AuthenticationPanelProps extends ContextAwareProps { }

interface AuthenticationPanelState
{
    email?: string;
    password?: string;
}

class AuthenticationPanelComponent extends React.Component<AuthenticationPanelProps, AuthenticationPanelState>
{
    constructor(props: AuthenticationPanelProps)
    {
        super(props);

        this.state = {
        };
    }

    public componentDidMount() { }

    public login()
    {
        const data = {
            login: this.state.email,
            password: this.state.password
        };

        AjaxService.put("api/login", data).then(() =>
        {
            this.props.context.setRoute(Routes.dashboard);
        }).catch((e) =>
        {
            console.log(e);
            console.log("oups");
        });
    }

    public render()
    {
        return <React.Fragment>
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
                            <input type="email" className="form-control" id="email" onChange={(e) => this.setState({ email: e.target.value })} />
                        </div>
                        <div className="form-group">
                            <label htmlFor="password" className="bmd-label-floating">Password</label>
                            <input type="password" className="form-control" id="password" onChange={(e) => this.setState({ password: e.target.value })} />
                        </div>
                        <div className="form-group text-right">
                            <button type="submit" className="btn btn-primary btn-raised" onClick={(e) =>
                            {
                                e.preventDefault();
                                this.login();
                            }}>Submit</button>
                        </div>
                    </form>
                </div>
            </div>
        </React.Fragment>;
    }
}

export const AuthenticationPanel = withContext<AuthenticationPanelProps>(AuthenticationPanelComponent);