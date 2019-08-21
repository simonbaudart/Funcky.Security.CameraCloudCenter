import React from "react";

import { ContextContent } from "./Models";
import { ContextProvider, Menu } from "./Components";
import { Route, Routes } from "./Routing";
import { AjaxService } from "./Services";

import { AuthenticationPanel, CameraPanel } from "./Containers";

import AppActions from "./Stores/AppActions";
import Events from "./Stores/Events";
import UserStore from "./Stores/UserStore";

interface ApplicationState
{
    context: ContextContent;
    userConnected: boolean;
}

export class Application extends React.Component<any, ApplicationState>
{
    constructor(props: any)
    {
        super(props);

        this.state = {
            context: {
                route: document.location.hash.replace("#", "") || "/",
                setRoute: this.setRoute.bind(this),
                updateContext: this.updateContext.bind(this),
            },
            userConnected: true
    };
    }

    public componentDidMount()
    {
        AppActions.loginSuccess();
        this.setRoute(Routes.dashboard);
        this.checkIdentity();

        UserStore.addChangeListener(Events.UserChanged, () => this.checkIdentity());
    }

    private checkIdentity()
    {
        AjaxService.get("api/isAuthenticated").then(() =>
            {
                if (!this.state.userConnected)
                {
                    this.setRoute(Routes.dashboard);
                    AppActions.loginSuccess();
                    this.setState({ userConnected: true });
                }
            })
            .catch((code: number) =>
            {
                if (code === 401)
                {
                    if (this.state.userConnected)
                    {
                        this.setRoute(Routes.login);
                        AppActions.logoutSuccess();
                        this.setState({ userConnected: false });
                    }
                }
            });
    }

    public updateContext(context: ContextContent)
    {
        this.setState({ context: context });
    }

    public setRoute = (route: string) =>
    {
        window.location.href = `#${route}`;
        const context = this.state.context;
        context.route = route;
        this.updateContext(context);
    };

    public render()
    {
        return <ContextProvider value={this.state.context}>
            <div className="container-fluid">
                <div className="row pb-3">
                    <div className="col">
                        <Menu />
                    </div>
                </div>

                <Route path={Routes.dashboard}>
                    <CameraPanel />
                </Route>

                <Route path={Routes.login}>
                    <AuthenticationPanel />
                </Route>
            </div>
        </ContextProvider>;
    }
}