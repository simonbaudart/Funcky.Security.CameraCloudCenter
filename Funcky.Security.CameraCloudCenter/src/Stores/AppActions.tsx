import Actions from "./Actions";
import AppDispatcher from "./AppDispatcher";

class AppActions
{
    public loginSuccess()
    {
        AppDispatcher.dispatch({
            actionType: Actions.LoginSuccess
        });
    }

    public logoutSuccess()
    {
        AppDispatcher.dispatch({
            actionType: Actions.LogoutSuccess
        });
    }
}

export default new AppActions()