import { SupplyMethodVm } from "./supplyMethodVm";

export class UserSupplyMethodVm {

    public Id: number;
    public PricePerUnit: number;
    public PricePerMany: number;
    public TimeInHours: number;
    public UserId: string;
    public SupplyMethodId: number;
    public IsActive: boolean;
    public SupplyMethod: SupplyMethodVm;

}