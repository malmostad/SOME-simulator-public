import Post from "@/Types/Admin/EditPost";
import EditComment from "@/Types/Admin/EditComment";
import EditPhase from "@/Types/Admin/EditPhase";

export default class EditScenario {
    public name: string = '';
    public description: string = '';
    
    public posts: Array<Post> = new Array<Post>();
    public comments: Array<EditComment> = new Array<EditComment>();
    public phases: Array<EditPhase> = new Array<EditPhase>();
}