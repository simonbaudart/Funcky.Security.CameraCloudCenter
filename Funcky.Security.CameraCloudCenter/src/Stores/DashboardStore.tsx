import AppDispatcher from "./AppDispatcher";
import { EventEmitter } from "events";

import { Camera } from "../Models/Camera";

import UserStore from "./UserStore";
import Events from "./Events";

import {AjaxService} from "../Services";

interface DashboardContent
{
    cameras: Camera[];
}

class DashboardStore extends EventEmitter
{
    private content: DashboardContent;

    constructor()
    {
        super();

        this.content = {
            cameras: []
        };

        AppDispatcher.register((payload) =>
        {
            switch (payload.actionType)
            {
                // TODO : fill with actions
            }
        });

        UserStore.addChangeListener(Events.UserChanged, this.loadCameras.bind(this));
    }

    public getContent(): DashboardContent
    {
        return this.content;
    }

    public addChangeListener(eventName, callback)
    {
        this.on(eventName, callback);
    }

    public removeChangeListener(eventName, callback)
    {
        this.removeListener(eventName, callback);
    }

    private loadCameras()
    {
        AjaxService.get<Camera[]>("/api/cameras").then((data) =>
            {
                this.content.cameras = data;
                this.emit(Events.CameraListLoaded);
            })
            .catch((code: number) =>
            {
                if (code === 401)
                {
                    // TODO : move it
                    // this.props.context.setRoute(Routes.login);
                }
            });
    }
}

export default new DashboardStore();