export default interface Dialog {
    open: boolean;
    title: string;
    content: string;
    confirmText: string;
    sender: string|null;
    newsEvent: boolean;
}
