import { Breadcrumbs, Button } from '@mantine/core';

import { AncestorInfo } from '@/lib/models';

export default function Navigator(props: {
  ancestors: AncestorInfo[];
  name: string;
  onClick: (id: number) => void;
}) {
  const { ancestors, name, onClick } = props;

  const breadcurmbs = ancestors.map((item) => (
    <Button
      key={item.id}
      variant="subtle"
      onClick={() => {
        onClick(item.id);
      }}
    >
      {item.name}
    </Button>
  ));
  return (
    <div className="flex items-center gap-2">
      <Breadcrumbs separator=">" separatorMargin="0" mt="xs">
        {breadcurmbs}
        <Button variant="transparent">{name}</Button>
      </Breadcrumbs>
    </div>
  );
}
