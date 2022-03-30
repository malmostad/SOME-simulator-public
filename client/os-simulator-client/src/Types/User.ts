import { Session } from './Session';
import { Scenario } from '../Types/Scenario';
import CurrentScenario from '../Types/CurrentScenario';
import { SessionLog } from '../Types/SessionLog';
import {MessageFlow} from '@/Types/MessageFlow';
import UserRole from "@/Types/UserRole";
import EditScenario from "@/Types/Admin/EditScenario";

export class User {
    public scenarios: any;
    public sessions: Session[];
    public activityLog: SessionLog[];
    public eventLog: SessionLog[];
    public sessionGroupId: number | null;
    public typeableCode: string | null;
    public currentScenario: CurrentScenario | null;
    public groupName: string | null;
    public currentEvent: string | null;
    public selectedFlow: number;
    public userRoles: UserRole[];
    public editScenarioId: number | null;
    public editScenario: EditScenario | null;

    public constructor() {
        this.scenarios = [];
        this.sessions = [];
        this.activityLog = [];
        this.eventLog = [];
        this.sessionGroupId = localStorage.getItem('sessionGroupId') != null ? parseInt( localStorage.getItem('sessionGroupId') || '-1' ): null;
        this.typeableCode = localStorage.getItem('typeableCode');
        this.currentScenario = null;
        this.currentEvent = '';
        this.groupName = localStorage.getItem('groupName');
        this.selectedFlow = MessageFlow.Short;
        this.userRoles = JSON.parse(<string>localStorage.getItem('userRoles'));
        this.editScenarioId = parseInt(localStorage.getItem('editScenarioId') || '');
        this.editScenario = null;
    }
}
