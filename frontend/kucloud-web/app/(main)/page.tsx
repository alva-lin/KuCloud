'use client';

import { useQuery } from '@tanstack/react-query';

import { ColorSchemeToggle } from '@/components/ColorSchemeToggle/ColorSchemeToggle';
import { Welcome } from '@/components/Welcome/Welcome';
import { Api } from '@/lib/api';

export default function HomePage() {
  const { data: folder } = useQuery({
    queryKey: ['folder', 1],
    queryFn: () => Api.Storage.getFolder({ id: 1 }),
  });

  return (
    <>
      <Welcome />
      <ColorSchemeToggle />
      {folder && <div>{JSON.stringify(folder)}</div>}
    </>
  );
}
