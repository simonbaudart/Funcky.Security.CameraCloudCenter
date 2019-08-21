import AppDispatcher from "./AppDispatcher";
import { EventEmitter } from "events";

class DashboardStore extends EventEmitter
{
    constructor()
    {
        super();

        AppDispatcher.register((payload) =>
        {
            switch (payload.actionType)
            {
                // TODO : fill with actions
            }
        });
    }
}

export default new DashboardStore();