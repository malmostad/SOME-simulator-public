import { SessionLog } from './SessionLog';

export interface Session {
    id: number;
    sessionGuid: string;
    sessionLogs: SessionLog[];
    participant: string;
}
