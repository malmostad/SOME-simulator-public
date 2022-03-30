import {MessageFlow} from '@/Types/MessageFlow';
import {SessionLog} from '@/Types/SessionLog';

export class Toast {
    public messageFlow: MessageFlow;
    public text: string;
    public ref: string;
    private id: number;
    private sender: string;
    public expanded: boolean;
    public descriptiveName: string;

    constructor(sessionLog: SessionLog) {
        this.ref = 'message' + sessionLog.id;
        this.text = sessionLog.messageType == 'ScenarioEvent' ? sessionLog.heading : sessionLog.text;
        this.messageFlow = sessionLog.messageFlow;
        this.id = sessionLog.id;
        this.sender = sessionLog.sender;
        this.expanded = true;
        this.descriptiveName = Toast.translate(sessionLog.messageType);
    }

    private static translate(messageType: string) {
        if (messageType === 'Comment') {
            return 'Ny kommentar';
        } 
        else if(messageType == 'ScenarioEvent') {
            return "Ny händelse"
        }
        else {
            return 'Nytt inlägg';
        }
    }
}
