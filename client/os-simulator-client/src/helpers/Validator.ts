export default class Validator {
    public static Enum(enumval: number|null|undefined) {
        return enumval && enumval > 0;
    }

    public static String(str: string|null|undefined) {
        return str && str.length > 0 ;
    }

    public static Array(arr: Array<any>|null|undefined) {
        return arr && arr.length > 0;
    }

    static WithinRange(val: number|null|undefined, min: number, max: number) {
        return  val != null &&  val >= min && val <= max;
    }
}