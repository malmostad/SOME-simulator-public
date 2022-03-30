import { SessionLogType } from './SessionLogType';
import { MessageFlow } from './MessageFlow';

export interface SessionLog {
    avatar: string;
    botReplyProperties: number;
    children: SessionLog[];
    heading: string;
    id: number;
    scenarioEventId: string;
    postId: string;
    sendDateTime: Date;
    sender: string;
    sessionId: string;
    sessionLogTag: number;
    text: string;
    messageType: string;
    messageFlow: MessageFlow;
    sessionGuid: string;
    level: number;
}

