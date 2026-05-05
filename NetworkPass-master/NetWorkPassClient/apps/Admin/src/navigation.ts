export interface NavigationModel{
  title:string;
  url:string;
  icon:string;
  subNavMenu?:boolean;
  subNavs?:NavigationModel[]
}
export const navigations:NavigationModel[]=[
  {
    title:"Idarəetmə Paneli",
    url:"/",
    icon:"bi-speedometer2"

  },{
    title:"Şöbələr",
    url:"/departments",
    icon:"bi-buildings-fill"
  }
]
