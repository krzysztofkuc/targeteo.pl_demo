import { Role } from "./role";
import { UserSupplyMethodVm } from "./userSupplyMethodVm";

export class UserLogin {

    constructor(public Token: string,
        public RefreshToken: string,
        public RefreshTokenExprarationDate: string,
        public Email: string,
        public UserName: string,
        public Password: string,
        public Login: string,
        public Id: string,
        public Roles: string[],
        public UserSupplyMethods: UserSupplyMethodVm[]) { }
}
