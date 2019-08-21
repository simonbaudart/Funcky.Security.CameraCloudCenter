import AppDispatcher from "./AppDispatcher";
import Actions from "./Actions";
import Events from "./Events";

import { EventEmitter } from "events";

class UserStore extends EventEmitter
{
    constructor()
    {
        super();

        AppDispatcher.register((payload) =>
        {
            switch (payload.actionType)
            {
                case Actions.LoginSuccess:
                case Actions.LogoutSuccess:
                    this.emit(Events.UserChanged);
                    break;
            }
        });
    }

    public addChangeListener(eventName, callback)
    {
        this.on(eventName, callback);
    }

    public removeChangeListener(eventName, callback)
    {
        this.removeListener(eventName, callback);
    }
}

export default new UserStore();