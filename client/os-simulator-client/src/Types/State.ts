import Dialog from './Dialog';
import { SessionGroup } from './SessionGroup';
import { User } from './User';
import { SessionLog } from './SessionLog';
import Alert from './Alert';

export default class State {
    public progress!: number; // Används bara av facilitator
    public status: string; // Används av båda
    public sessionGroups: SessionGroup[]; //
    public user: User;
    public session: any | null;
    public dialog: Dialog;
    public signedIn: boolean;
    public duration: string | null;
    public show: SessionLog | null = null;
    public stressLevel: number | null;
    public shortRead: number = 0;
    public longRead: number = 0;
    public alert: Alert | null = null;
    public newSessionLogs: number[] = [];

    public constructor() {
        this.signedIn = localStorage.getItem('signedIn') === 'true' || false;
        this.progress = 0;
        this.duration = localStorage.getItem('duration');
        this.status = '';
        this.sessionGroups = [];
        this.user = new User();
        this.session = {
            sessionId: localStorage.getItem('sessionId') || '',
            messages: [],
            participant: localStorage.getItem('participant'),
        };
        this.dialog = {
            open: false,
            title: 'title',
            content: 'message',
            confirmText: 'Ok',
            newsEvent: false,
            sender: '',
        };

        this.alert = null;

        this.stressLevel = 50;
        this.shortRead = parseInt(localStorage.getItem('shortRead') || '0', 10);
        this.longRead = parseInt(localStorage.getItem('longRead') || '0', 10);
    }
}
