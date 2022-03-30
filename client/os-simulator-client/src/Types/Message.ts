import { SessionLog } from './SessionLog';

export default interface Message {
    messageCount: number;
    sessionLog: SessionLog;
    sessionGroup: string;
}
