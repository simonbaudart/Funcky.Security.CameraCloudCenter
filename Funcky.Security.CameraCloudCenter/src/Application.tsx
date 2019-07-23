import React from "react";

import { Camera, ContextContent } from "./Models";
import { AjaxService } from "./Services";
import { CameraDetail, CameraList, ContextProvider, AuthenticationPanel, Menu } from "./Components";
import { Route, Routes } from "./Routing";

interface ApplicationState
{
    context: ContextContent;
    cameras: Camera[];
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
                currentCamera: undefined
            },
            cameras: []
        };
    }

    public componentDidMount()
    {
        this.loadCameras();
    }

    public updateContext(context: ContextContent)
    {
        this.setState({ context: context });
    }

    public setRoute = (route: string) =>
    {
        window.location.href = `#${route}`;
        var context = this.state.context;
        context.route = route;
        this.updateContext(context);

        if (route === Routes.dashboard)
        {
            this.loadCameras();
        }
    };

    private displayCameraDetail()
    {
        if (this.state.context.currentCamera)
        {
            return <CameraDetail camera={this.state.context.currentCamera}/>;
        }
        return <div></div>;
    }

    public loadCameras()
    {
        AjaxService.get<Camera[]>("/api/cameras").then((data) =>
            {
                this.setState({ cameras: data });
            })
            .catch((code: number) =>
            {
                if (code === 401)
                {
                    this.state.context.setRoute(Routes.login);
                }
            });
    }

    public render()
    {
        return <ContextProvider value={this.state.context}>
                   <div className="container-fluid">
                       <div className="row pb-3">
                           <div className="col">
                               <Menu/>
                           </div>
                       </div>

                       <Route path={Routes.dashboard}>
                           <CameraList cameras={this.state.cameras}/>

                           {this.displayCameraDetail()}
                       </Route>

                       <Route path={Routes.login}>
                           <AuthenticationPanel/>
                       </Route>
                   </div>
               </ContextProvider>;
    }
}