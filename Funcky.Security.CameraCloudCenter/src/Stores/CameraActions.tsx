import Actions from "./Actions";
import AppDispatcher from "./AppDispatcher";

import {Camera} from "Models";

class CameraActions
{
    public selectCamera(camera: Camera)
    {
        AppDispatcher.dispatch({
            actionType: Actions.CameraSelect,
            data: camera
        });
    }
}

export default new CameraActions()