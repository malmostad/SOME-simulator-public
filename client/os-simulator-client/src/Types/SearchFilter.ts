import { SessionLog } from './SessionLog';
import { MessageFlow } from './MessageFlow';

export default class SearchFilter {

    constructor(messageFlow: MessageFlow|null = null) {
        this.messageFlow = messageFlow;
    }

    public sessionId: number | null = null;
    public messageFlow: MessageFlow | null = null;

    public filter( sessionLogs: SessionLog[] ): SessionLog[] {
        let tmpSessionLog = sessionLogs;
        
        if (!sessionLogs) {
            return [];
        }
        
        if (this.sessionId !== null) {
            tmpSessionLog =  tmpSessionLog.filter((s) => {
                return s.sessionId.toString() === this.sessionId!.toString();
            });
        }
        if (this.messageFlow !== null && this.messageFlow > 0) {
            tmpSessionLog =  tmpSessionLog.filter((s) => {
                return  !s.messageFlow || s.messageFlow.toString() === this.messageFlow!.toString();
            });
        }

        return [...tmpSessionLog];
    }
}
