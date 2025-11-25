export interface selectedButtonOption {
  value: string | number;
  name: string | number;
}
export interface selectedButtonModel {
  iconName?: string;
  labelName?: string;
  isSelectedIcon?: boolean;
  options?: selectedButtonOption[];
  position?: string;
  defaultWidth?: string;
}
