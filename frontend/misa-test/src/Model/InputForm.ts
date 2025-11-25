export interface InpurForm {
  key: string;
  label: string;
  type: "input" | "textarea" | "select" | "date" | "email" | "phone";
  options?: { value: string; name: string }[];
  isDisabled?: boolean;
  require?: boolean;
}
