export default class Alert {
    public content: string | null;
    public title: string | null;

    constructor(title: string|null = null, content: string|null = null) {
        this.title = title;
        this.content = content;
    }
}
