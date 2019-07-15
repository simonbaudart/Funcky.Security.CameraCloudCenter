import React from "react";
import { ContextContent} from "./Models/ContextContent";
import { Camera } from "./Models/Camera";
import { AjaxService } from "./Services/AjaxService";
import { ContextProvider } from "./Components/Shared/Context";

import { CameraList } from "./Components/Camera/CameraList";

interface ApplicationState {
    context: ContextContent;
    cameras: Camera[];
}

export class Application extends React.Component<any, ApplicationState>
{
    constructor(props: any) {
        super(props);

        this.state = {
            context: {
                route: document.location.hash.replace("#", "") || "/",
                setRoute: this.setRoute.bind(this),
                updateContext: this.updateContext.bind(this),
            },
            cameras: []
        };
    }

    public componentDidMount()
    {
        AjaxService.get<Camera[]>('/api/cameras').then((data) =>
        {
            this.setState({ cameras: data });
        });
    }

    public updateContext(context: ContextContent) {
        this.setState({ context: context });
    }

    public setRoute = (route: string) => {
        window.location.href = '#' + route;
        var context = this.state.context;
        context.route = route;
        this.updateContext(context);
    };

    public render(): Object | string | number | {} | Object | Object | boolean | null | undefined {
        return <ContextProvider value={this.state.context}>
            <div className="container-fluid">
                <div className="row pb-3">
                    <div className="col">
                        <menu />
                    </div>
                </div>

                <div className="row pb-3">
                    <CameraList cameras={this.state.cameras} />
                </div>
                
            </div>
        </ContextProvider>;
    }
}