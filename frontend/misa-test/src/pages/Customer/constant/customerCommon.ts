import type { InpurForm } from "../../../Model/InputForm";
import { customerType } from "./customerType";

export const inputItems: InpurForm[] = [
  {
    key: "customer_code",
    label: "Mã khách hàng",
    type: "input",
    isDisabled: true,
  },
  {
    key: "full_name",
    label: "Tên khách hàng ",
    type: "input",
    require: true,
  },
  { key: "phone", label: "Điện thoại  ", type: "phone", require: true },
  {
    key: "customer_type",
    label: "Loại khách hàng",
    type: "select",
    options: customerType.slice(1),
  },
  {
    key: "address",
    label: "Sổ hộ chiếu ",
    type: "input",
  },
  {
    key: "email",
    label: "Email ",
    type: "email",
    require: true,
  },
  {
    key: "tax_code",
    label: "Mã số thuế",
    type: "input",
    require: true,
  },
  {
    key: "shipping_address",
    label: "Địa chỉ giao hàng",
    type: "input",
  },
  {
    key: "billing_address",
    label: "Địa chỉ thanh toán",
    type: "input",
  },
  {
    key: "zalo",
    label: "Zalo",
    type: "input",
  },
];
