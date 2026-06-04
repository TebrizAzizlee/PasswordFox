export interface NavigationModel{
  title:string;
  url:string;
  icon:string;
  subNavMenu?:boolean;
  subNavs?:NavigationModel[]
}
export const navigations:NavigationModel[]=[
  {
    title:"Dashboard",
    url:"/",
    icon:"bi-speedometer2"

  },{
    title:"Branches",
    url:"/departments",
    icon:"bi-buildings-fill"
  },
  {
    title:"Devices",
    url:"/devices",
    icon:"bi bi-display"
  }
]
