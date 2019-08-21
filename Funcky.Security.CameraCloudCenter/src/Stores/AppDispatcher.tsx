import Flux from 'flux';

interface DispatcherPayload
{
    actionType: string;
    data?: any;
}

export default new Flux.Dispatcher<DispatcherPayload>();