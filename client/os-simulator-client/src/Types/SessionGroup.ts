export interface SessionGroup {
    duration: string | null;
    GroupName: string | null;
    id: number | null;
    scenarioId: number | null;
    scenarioName: string;
    sessions: any[];
    startDate: string | null;
    status: string | null;
    stopDate: string | null;
    stressLevel: number;
    typeableCode: string | null;
}
